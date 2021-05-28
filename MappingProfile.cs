using AutoMapper;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSalvador
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {

            CreateMap<citas, citaDTO>()
                .ForMember(dest => dest.medico_nombre, opt => opt.MapFrom(src => src.medicos.nombre))
                .ForMember(dest => dest.medico_apellido, opt => opt.MapFrom(src => src.medicos.apellido))
                .ForMember(dest => dest.paciente_nombre, opt => opt.MapFrom(src => src.pacientes.nombre))
                .ForMember(dest => dest.paciente_apellido, opt => opt.MapFrom(src => src.pacientes.apellido))
                .ForMember(dest => dest.paciente_nombre_tutor, opt => opt.MapFrom(src => src.pacientes.nombre_tutor))
                .ForMember(dest => dest.paciente_apellido_tutor, opt => opt.MapFrom(src => src.pacientes.apellido_tutor))
                .ForMember(dest => dest.servicio_descrip, opt => opt.MapFrom(src => src.servicios.descrip))
                .ForMember(dest => dest.seguro_descrip, opt => opt.MapFrom(src => src.seguros.descrip))
                .ForMember(dest => dest.especialidad_descrip, opt => opt.MapFrom(src => src.especialidades.descrip));
         

            CreateMap<pacientes, pacienteDTO>();
            CreateMap<pacienteDTO, pacientes>();
            CreateMap<citaCreateDTO, pacientes>();
            CreateMap<RegisterDTO, MyIdentityUser>()
               .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));

            CreateMap<medicos, medico_directorioDTO>()
                .ForMember(dest => dest.especialidades, opt => opt.MapFrom(src => 
                    src.especialidades_medicos.Select(x => x.especialidades).ToList()
                    ));


        }
    }
}
