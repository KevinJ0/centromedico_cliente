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
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {

            CreateMap<pruebas, pruebaDTO>()
                .ForMember(dest => dest.descrip, opt => opt.MapFrom(src => src.analisis.descrip));

            CreateMap<pruebaDTO, pruebas>();

            CreateMap<citas, citaDTO>()
                .ForMember(dest => dest.medico_nombre, opt => opt.MapFrom(src => src.medicos.nombre))
                .ForMember(dest => dest.medico_apellido, opt => opt.MapFrom(src => src.medicos.apellido))
                .ForMember(dest => dest.paciente_nombre, opt => opt.MapFrom(src => src.pacientes.nombre))
                .ForMember(dest => dest.paciente_apellido, opt => opt.MapFrom(src => src.pacientes.apellido))
                .ForMember(dest => dest.paciente_nombre_tutor, opt => opt.MapFrom(src => src.pacientes.nombre_tutor))
                .ForMember(dest => dest.paciente_apellido_tutor, opt => opt.MapFrom(src => src.pacientes.apellido_tutor))
                .ForMember(dest => dest.servicio_descrip, opt => opt.MapFrom(src => src.servicios.descrip))
                .ForMember(dest => dest.seguro_descrip, opt => opt.MapFrom(src => src.seguros.descrip));
              //  .ForMember(dest => dest.especialidad_descrip, opt => opt.MapFrom(src => src.especialidades.descrip));


            CreateMap<pacientes, pacienteDTO>();
            CreateMap<pacienteDTO, pacientes>();
            CreateMap<citaCreateDTO, pacientes>();
            CreateMap<RegisterDTO, MyIdentityUser>()
               .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.UserCredential));

            CreateMap<medicos, medicoDirectorioDTO>()
                .ForMember(dest => dest.especialidades, opt => opt.MapFrom(src =>
                    src.especialidades_medicos.Select(x => x.especialidades.descrip).ToList()));


            CreateMap<medicos, medicoDTO>()
         .ForMember(dest => dest.especialidades, opt => opt.MapFrom(src =>
             src.especialidades_medicos.Select(x => x.especialidades.descrip).ToList()
             )).ForMember(dest => dest.seguros, opt => opt.MapFrom(src =>
            src.cobertura_medicos.Select(x => x.seguros.descrip).ToList()
            )).ForMember(dest => dest.servicios, opt => opt.MapFrom(src =>
            src.servicios_medicos.Where(x=> x.medicos.ID == src.ID ).Select(x=> x.servicios.descrip).ToList()
            ));
        }


    }
}
