window.voltaMap = {
    map: null,
    pickMarker: null,
    dropMarker: null,
    routeLayer: null,
    dotNetRef: null,

    // 1. Initialize Map
    initMap: function (elementId, dotNetReference) {
        this.dotNetRef = dotNetReference;
        // Start map at Karachi
        this.map = L.map(elementId).setView([24.8607, 67.0011], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(this.map);

        // Listen for clicks
        this.map.on('click', async function (e) {
            const lat = e.latlng.lat;
            const lng = e.latlng.lng;

            // Reverse Geocode (Get address from clicks)
            const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`);
            const data = await response.json();
            const address = data.display_name.split(',')[0]; // Short address

            // Call C# back
            dotNetReference.invokeMethodAsync('OnMapClicked', lat, lng, address);
        });
    },

    // 2. Add Pins (Green for Pickup, Red for Dropoff)
    addMarker: function (lat, lng, isPickup) {
        if (isPickup) {
            if (this.pickMarker) this.map.removeLayer(this.pickMarker);
            this.pickMarker = L.marker([lat, lng]).addTo(this.map)
                .bindPopup("Pickup").openPopup();
        } else {
            if (this.dropMarker) this.map.removeLayer(this.dropMarker);
            this.dropMarker = L.marker([lat, lng]).addTo(this.map)
                .bindPopup("Dropoff").openPopup();
        }
        this.map.setView([lat, lng], 14);
    },

    // 3. Search (Geocoding)
    searchLocation: async function (query, isPickup) {
        const response = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${query}+Karachi`);
        const data = await response.json();

        if (data && data.length > 0) {
            const lat = data[0].lat;
            const lng = data[0].lon;
            const address = data[0].display_name.split(',')[0];

            this.addMarker(lat, lng, isPickup);
            return `${lat},${lng}`; // Return to C#
        }
        return null;
    },

    // 4. Calculate Route (OSRM API)
    calculateRoute: async function (pickLat, pickLng, dropLat, dropLng) {
        // Clear old line
        if (this.routeLayer) this.map.removeLayer(this.routeLayer);

        const url = `https://router.project-osrm.org/route/v1/driving/${pickLng},${pickLat};${dropLng},${dropLat}?overview=full&geometries=geojson`;

        try {
            const response = await fetch(url);
            const data = await response.json();

            if (data.routes && data.routes.length > 0) {
                const route = data.routes[0];
                const coordinates = route.geometry.coordinates.map(c => [c[1], c[0]]); // Swap for Leaflet

                // Draw Blue Line
                this.routeLayer = L.polyline(coordinates, { color: 'blue', weight: 5 }).addTo(this.map);
                this.map.fitBounds(this.routeLayer.getBounds());

                // Calculate Stats
                const minutes = Math.round(route.duration / 60);
                const km = (route.distance / 1000).toFixed(1);

                return `${minutes} mins|${km} km`; // Return to C#
            }
        } catch (e) {
            console.error("Routing failed", e);
        }
        return null;
    }
};