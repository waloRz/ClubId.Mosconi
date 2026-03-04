
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

        // $(document).ready(function () {
        //     // Solo activamos DataTables si hay datos
        //     if ($('#tablaBoletines tbody tr').length > 0 && !$('#tablaBoletines td[colspan]').length) {
        //         $('#tablaBoletines').DataTable({
        //             language: {
        //                url: 'https://cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json'
        //                 // Español automático
        //             },
        //             order: [[0, 'desc']], // Ordenar por fecha (primera columna) descendente- desc
        //             pageLength: 10 
        //         });
        //     }
        // });

