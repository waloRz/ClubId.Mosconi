
        // Función para aplicar el formato del DNI (mantener tu código original)
        function formatAndValidateDni(input) {
            // ... (Tu lógica de formato de DNI aquí) ...
            let dniValue = input.value.replace(/\D/g, '');
            let formattedDni = '';
            
            if (dniValue.length > 0) {
                formattedDni += dniValue.substring(0, 2);
            }
            if (dniValue.length > 2) {
                formattedDni += '.' + dniValue.substring(2, 5);
            }
            if (dniValue.length > 5) {
                formattedDni += '.' + dniValue.substring(5, 8);
            }

            if (formattedDni.length > 10) {
                formattedDni = formattedDni.substring(0, 10);
            }
            input.value = formattedDni;
        }

        // // --- LÓGICA DE CASCADA DE DROPDOWNS ---
        // $(document).ready(function () {
        //     // 1. Obtener las referencias a los selects
        //     const categoriaSelect = $('#IdCategoria');
        //     const equipoSelect = $('#IdEquipo');

        //     // 2. Manejar el evento 'change' del select de Categoría
        //     categoriaSelect.change(function () {
        //         const categoriaId = $(this).val();
                
        //         // Limpiar y deshabilitar el select de Equipo mientras carga
        //         equipoSelect.empty().prop('disabled', true);
        //         equipoSelect.append('<option value="">Cargando...</option>');

        //         if (categoriaId) {
        //             // Realizar llamada AJAX
        //             $.ajax({
        //                 url: '@Url.Action("GetEquiposPorCategoria", "Jgrxeqp")', // Asegúrate que el Controller sea "Jugador"
        //                 type: 'GET',
        //                 data: { idCategoria: categoriaId },
        //                 dataType: 'json',
        //                 success: function (data) {
        //                     // Limpiar y agregar la opción por defecto
        //                     equipoSelect.empty().prop('disabled', false);
        //                     equipoSelect.append('<option value="">Seleccione un EQUIPO</option>');

        //                     if (data.length > 0) {
        //                         // Agregar los nuevos equipos recibidos del Controller
        //                         $.each(data, function (key, equipo) {
        //                             equipoSelect.append('<option value="' + equipo.value + '">' + equipo.text + '</option>');
        //                         });
        //                     } else {
        //                         equipoSelect.append('<option value="">No hay equipos en esta categoría</option>');
        //                     }
        //                 },
        //                 error: function () {
        //                     equipoSelect.empty().prop('disabled', false);
        //                     equipoSelect.append('<option value="">Error al cargar equipos</option>');
        //                 }
        //             });
        //         } else {
        //             // Si no se selecciona Categoría, limpiar el select de Equipo
        //             equipoSelect.empty().prop('disabled', false);
        //             equipoSelect.append('<option value="">Seleccione un EQUIPO</option>');
        //         }
        //     });

        //     // Si se carga la página con una categoría seleccionada (ej. en edición), forzar el cambio.
        //     if (categoriaSelect.val() !== "") {
        //          categoriaSelect.trigger('change');
        //     }
            
        //     // Si estás usando jQuery Validation
        //     $.validator.unobtrusive.parse(document);
        // });
