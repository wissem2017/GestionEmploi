import { Photo } from "./photo";

export interface User {
    id:number;
    userName:string;
    knownAs:string;
    age:number;
    geder:string;
    created:Date;
    lastActive:String;
    photoUrl:string;
    city:string;
    country:string;
    interests?:string;
    intoduction?:string;
    lookingFor?:string;
    photos?:Photo[];
    roles?:string[];
}
