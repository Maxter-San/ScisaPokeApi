document.addEventListener('DOMContentLoaded', function () {
    const modalDetalle = document.getElementById('modalDetallePokemon');

    if (modalDetalle) {
        modalDetalle.addEventListener('show.bs.modal', async function (event) {
            const triggerRow = event.relatedTarget;
            const pokemonId = triggerRow.getAttribute('data-id');

            const spinner = document.getElementById('loadingSpinner');
            const content = document.getElementById('modalContent');

            spinner.style.display = 'block';
            content.style.display = 'none';

            try {
                const response = await fetch(`/Pokemon/GetDetail?id=${pokemonId}`);
                
                if (!response.ok) {
                    throw new Error('No se pudo obtener la información del Pokémon.');
                }

                const data = await response.json();

                document.getElementById('formDetalleNombre').value = data.name;
                document.getElementById('formDetalleNumero').value = data.id;
                document.getElementById('formDetalleAltura').value = `${data.height} m`;
                document.getElementById('formDetallePeso').value = `${data.weight} kg`;
                document.getElementById('imgDetalleSprite').src = data.imageUrl;

                const typesContainer = document.getElementById('modalTypesContainer');
                typesContainer.innerHTML = '';
                
                data.types.forEach(typeName => {
                    const badge = document.createElement('span');
                    badge.className = 'badge text-bg-success me-1 text-capitalize';
                    badge.textContent = typeName;
                    typesContainer.appendChild(badge);
                });

                spinner.style.display = 'none';
                content.style.display = 'block';

            } catch (error) {
                console.error('Error en la petición:', error);
                spinner.innerHTML = `
                    <div class="alert alert-danger mb-0" role="alert">
                        Ocurrió un error al cargar el detalle. Intente nuevamente.
                    </div>
                `;
            }
        });
    }
});