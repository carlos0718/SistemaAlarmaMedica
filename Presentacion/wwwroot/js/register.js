/* ========================================
   REGISTRO DINÁMICO - Gestión de Formularios
   ======================================== */

/**
 * Inicializa el formulario dinámico de registro
 * Muestra/oculta campos según el tipo de usuario seleccionado
 *
 * @param {string} selectId - ID del select de tipo de usuario
 */
var InitializeRegistrationForm = function(selectId = 'tipoUsuarioSelect') {
    var tipoSelect = document.getElementById(selectId);

    if (!tipoSelect) {
        console.error('No se encontró el elemento con ID: ' + selectId);
        return;
    }

    // Elementos de las secciones
    var seccionPaciente = document.getElementById('seccionPaciente');
    var seccionMedico = document.getElementById('seccionMedico');

    // Campos de Paciente
    var documentoPacienteInput = document.getElementById('DocumentoPaciente');
    var apellidoPacienteInput = document.getElementById('ApellidoPaciente');
    var nombrePacienteInput = document.getElementById('NombrePaciente');

    // Campos de Médico
    var matriculaMedicoInput = document.getElementById('MatriculaMedico');
    var apellidoMedicoInput = document.getElementById('ApellidoMedico');
    var nombreMedicoInput = document.getElementById('NombreMedico');
    var especialidadInput = document.getElementById('EspecialidadId');

    /**
     * Actualiza la visibilidad de secciones según el tipo de usuario
     */
    function actualizarSecciones() {
        var tipoSeleccionado = tipoSelect.value;

        // Limpiar validaciones anteriores
        limpiarValidacionesCampos();

        if (tipoSeleccionado === '3' || tipoSeleccionado === 'PACIENTE') {
            // Es Paciente
            mostrarSectionPaciente();
        } else if (tipoSeleccionado === '2' || tipoSeleccionado === 'MEDICO') {
            // Es Médico
            mostrarSectionMedico();
        } else {
            // No seleccionado
            ocultarTodasLasSecciones();
        }
    }

    /**
     * Muestra la sección de Paciente y marca campos como requeridos
     */
    function mostrarSectionPaciente() {
        if (seccionPaciente) seccionPaciente.style.display = 'block';
        if (seccionMedico) seccionMedico.style.display = 'none';

        // Marcar como requeridos
        if (documentoPacienteInput) documentoPacienteInput.required = true;
        if (apellidoPacienteInput) apellidoPacienteInput.required = true;
        if (nombrePacienteInput) nombrePacienteInput.required = true;

        // Desmarcar campos médico
        if (matriculaMedicoInput) matriculaMedicoInput.required = false;
        if (apellidoMedicoInput) apellidoMedicoInput.required = false;
        if (nombreMedicoInput) nombreMedicoInput.required = false;
        if (especialidadInput) especialidadInput.required = false;
    }

    /**
     * Muestra la sección de Médico y marca campos como requeridos
     */
    function mostrarSectionMedico() {
        if (seccionPaciente) seccionPaciente.style.display = 'none';
        if (seccionMedico) seccionMedico.style.display = 'block';

        // Desmarcar campos paciente
        if (documentoPacienteInput) documentoPacienteInput.required = false;
        if (apellidoPacienteInput) apellidoPacienteInput.required = false;
        if (nombrePacienteInput) nombrePacienteInput.required = false;

        // Marcar como requeridos
        if (matriculaMedicoInput) matriculaMedicoInput.required = true;
        if (apellidoMedicoInput) apellidoMedicoInput.required = true;
        if (nombreMedicoInput) nombreMedicoInput.required = true;
        if (especialidadInput) especialidadInput.required = true;
    }

    /**
     * Oculta todas las secciones dinámicas
     */
    function ocultarTodasLasSecciones() {
        if (seccionPaciente) seccionPaciente.style.display = 'none';
        if (seccionMedico) seccionMedico.style.display = 'none';

        // Desmarcar todos como requeridos
        if (documentoPacienteInput) documentoPacienteInput.required = false;
        if (apellidoPacienteInput) apellidoPacienteInput.required = false;
        if (nombrePacienteInput) nombrePacienteInput.required = false;
        if (matriculaMedicoInput) matriculaMedicoInput.required = false;
        if (apellidoMedicoInput) apellidoMedicoInput.required = false;
        if (nombreMedicoInput) nombreMedicoInput.required = false;
        if (especialidadInput) especialidadInput.required = false;
    }

    /**
     * Limpia las validaciones de los campos
     */
    function limpiarValidacionesCampos() {
        var campos = [
            documentoPacienteInput, apellidoPacienteInput, nombrePacienteInput,
            matriculaMedicoInput, apellidoMedicoInput, nombreMedicoInput, especialidadInput
        ];

        campos.forEach(function(campo) {
            if (campo) {
                campo.classList.remove('is-invalid');
            }
        });
    }

    // Event listener para cambios en tipo de usuario
    tipoSelect.addEventListener('change', actualizarSecciones);

    // Inicializar al cargar la página si hay un tipo seleccionado
    document.addEventListener('DOMContentLoaded', function() {
        if (tipoSelect.value) {
            actualizarSecciones();
        }
    });

    // Si el DOM ya está listo (script cargado después de DOM)
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', actualizarSecciones);
    } else {
        actualizarSecciones();
    }
};

/**
 * Valida que los campos dinámicos requeridos estén completos
 * Utilizado en validación del lado del cliente antes de enviar
 *
 * @returns {boolean} true si la validación es correcta
 */
var ValidateRegistrationDynamicFields = function() {
    var tipoSelect = document.getElementById('tipoUsuarioSelect');
    var tipoSeleccionado = tipoSelect.value;
    var isValid = true;

    if (tipoSeleccionado === '3' || tipoSeleccionado === 'PACIENTE') {
        // Validar campos de paciente
        var documentoPaciente = document.getElementById('DocumentoPaciente');
        var apellidoPaciente = document.getElementById('ApellidoPaciente');
        var nombrePaciente = document.getElementById('NombrePaciente');

        if (!documentoPaciente.value.trim()) {
            marcarCampoInvalido(documentoPaciente);
            isValid = false;
        }
        if (!apellidoPaciente.value.trim()) {
            marcarCampoInvalido(apellidoPaciente);
            isValid = false;
        }
        if (!nombrePaciente.value.trim()) {
            marcarCampoInvalido(nombrePaciente);
            isValid = false;
        }
    } else if (tipoSeleccionado === '2' || tipoSeleccionado === 'MEDICO') {
        // Validar campos de médico
        var matriculaMedico = document.getElementById('MatriculaMedico');
        var apellidoMedico = document.getElementById('ApellidoMedico');
        var nombreMedico = document.getElementById('NombreMedico');
        var especialidad = document.getElementById('EspecialidadId');

        if (!matriculaMedico.value.trim()) {
            marcarCampoInvalido(matriculaMedico);
            isValid = false;
        }
        if (!apellidoMedico.value.trim()) {
            marcarCampoInvalido(apellidoMedico);
            isValid = false;
        }
        if (!nombreMedico.value.trim()) {
            marcarCampoInvalido(nombreMedico);
            isValid = false;
        }
        if (!especialidad.value) {
            marcarCampoInvalido(especialidad);
            isValid = false;
        }
    }

    return isValid;
};

/**
 * Marca un campo como inválido (agrega clase is-invalid y estilo)
 *
 * @param {HTMLElement} elemento - Elemento a marcar como inválido
 */
var marcarCampoInvalido = function(elemento) {
    if (elemento) {
        elemento.classList.add('is-invalid');
        elemento.style.borderColor = '#dc3545';
    }
};

/**
 * Limpia la marca de inválido de un campo
 *
 * @param {HTMLElement} elemento - Elemento a limpiar
 */
var limpiarMarcaInvalido = function(elemento) {
    if (elemento) {
        elemento.classList.remove('is-invalid');
        elemento.style.borderColor = '';
    }
};

/* ========================================
   FIN - REGISTRO DINÁMICO
   ======================================== */
