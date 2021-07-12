export interface UserInfo {

    doc_identidad: string
    nombre: string
    apellido: string
    confirm_doc_identidad: boolean
    contacto: string
    fecha_nacimiento: Date
    sexo: string

}
export interface especialidad {
    value: number;
    viewValue: string;
}
export interface hora {
    value: Date;
    viewValue: string;
}
export interface seguro {
    value: number;
    viewValue: string;
}

export interface cobertura {
    porciento: number;
    pago: number;
    cobertura: number;
    diferencia: number;

}



export interface cita {
    nombre: string;
    apellido: string;
    sexo: string;
    doc_identidad: string;
    fecha_hora: Date;
    medicosID: number;
    serviciosID: number;
    fecha_nacimiento: Date;
    contacto: string;
    contacto_whatsapp: boolean;
    appoiment_type: number;
    segurosID: number;
    nota: string;
}