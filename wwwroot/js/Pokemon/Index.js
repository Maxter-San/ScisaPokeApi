const modalDetalle = document.getElementById('modalDetallePokemon');

modalDetalle.addEventListener('show.bs.modal', function (event) {
    const triggerElement = event.relatedTarget; //row que activa el modal

    const id = triggerElement.getAttribute('data-id');
    const nombre = triggerElement.getAttribute('data-nombre');
    const altura = triggerElement.getAttribute('data-altura');
    const peso = triggerElement.getAttribute('data-peso');
    const imagen = triggerElement.getAttribute('data-imagen');

    modalDetalle.querySelector('#formDetalleNombre').value = nombre;
    modalDetalle.querySelector('#formDetalleNumero').value = id;
    modalDetalle.querySelector('#formDetalleAltura').value = altura;
    modalDetalle.querySelector('#formDetallePeso').value = peso;
    modalDetalle.querySelector('img').src = imagen;
});