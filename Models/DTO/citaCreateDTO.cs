using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalSalvador.Models.DTO
{
    public class citaCreateDTO
    {
        private DateTime _fechaNacimiento;
        private string _docIdentidadTutor;
        private string _docIdentidad;
        private string _nombreTutor;

        int? _edad;
        [Required]
        public int medicosID { get; set; }
        [StringLength(2)]
        public string? tipo_contacto { get; set; }
        [Required]
        public int? serviciosID { get; set; }
        [Column(TypeName = "text")]
        public string nota { get; set; }
        [StringLength(10)]
        public string contacto { get; set; }
        public bool? contacto_whatsapp { get; set; }
        public bool? contacto_llamada { get; set; }
        [Column(TypeName = "datetime")]
        [Required]
        public DateTime fecha_hora { get; set; }
        public int? segurosID { get; set; }
        [Required]
        public int? especialidadesID { get; set; }
        public string? cod_verificacionID { get; set; }


        //Patient
        [Required]
        public int appoiment_type { get; set; }
        [Required]
        [StringLength(40)]
        public string nombre { get; set; }
        [StringLength(50)]
        public string apellido { get; set; }
        [Required]
        [StringLength(1)]
        public string sexo { get; set; }

        [Column(TypeName = "date")]
        public DateTime fecha_nacimiento
        {
            get => _fechaNacimiento;
            set
            {
                int lessPermititted = DateTime.Compare(value, new DateTime(1910, 1, 1));
                int graterThanToday = DateTime.Compare(value, DateTime.Today);

                if (lessPermititted < 0 || graterThanToday > 0)
                {
                    throw new ArgumentOutOfRangeException("La fecha de nacimiento está fuera de rango permitido.");
                }
                else
                {
                    _fechaNacimiento = value;
                }
            }

        }
        [StringLength(15)]
        public string? doc_identidad
        {

            get => _docIdentidad;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    _docIdentidad = null;
                else
                {
                    _docIdentidad = value.Trim();
                 }
            }
        }
        [StringLength(40)]
        public string nombre_tutor
        {
            get => _nombreTutor;

            set
            {

                if (String.IsNullOrWhiteSpace(value) && doc_identidad_tutor != null)
                {
                    throw new ArgumentException("El nombre del tutor es requerido");
                }
                _nombreTutor = value;
            }
        }
        [StringLength(45)]
        public string apellido_tutor { get; set; }
        public bool extranjero { get; set; }
        [StringLength(10)]
        public string telefono { get; set; }
        [StringLength(40)]
        public string correo { get; set; }
        [StringLength(15)]
        public string doc_identidad_tutor
        {
            get => _docIdentidadTutor;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    _docIdentidadTutor = null;
                else
                {
                    
                    _docIdentidadTutor = value.Trim();
                }
            }
        }
        public int? edad
        {
            set { _edad = value; }
            get
            {
                _edad = DateTime.Today.AddTicks(-_fechaNacimiento.Ticks).Year - 1;


                if (_edad >= 18 && doc_identidad == null)
                    throw new Exception("No se ha provisto de un documento de identidad.");
                else if (_edad < 18 && doc_identidad_tutor == null ) // porque todos los menores de edad necesitan un tutor
                    throw new Exception("No se ha provisto de una identificación por parte del tutor que acompaña al paciente menor de edad.");
                else if (_edad < 18 && appoiment_type != 1) // porque todos los menores de edad necesitan un tutor
                    throw new Exception("Tipo de cita escogido erroneo para un menor de edad.");
                else if (_edad >= 18 && appoiment_type != 0) // porque todos los menores de edad necesitan un tutor
                    throw new Exception("Tipo de cita escogido erroneo para un mayor de edad.");
                else if (_edad < 18 && doc_identidad_tutor != null)
                    doc_identidad = null;
                else if(_edad >= 18)
                    doc_identidad_tutor = null;

                if (_edad < 120 && _edad > 0) return _edad;

                if (_edad == 0)
                {
                    _edad = DateTime.Today.AddTicks(-_fechaNacimiento.Ticks).Month;

                    if (_edad >= 1 && _edad <= 12)
                    {
                        menor_un_año = true;
                        return _edad - 1;
                    }
                }
                return null;
            }
        }
        public bool menor_un_año { get; set; }

       

    }
}
