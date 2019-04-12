import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  baseUrl='http://localhost:5000/api/auth/'; //--> URL Controleur API auth
constructor(private http:HttpClient) { }

//--> Méthode qui permet d'accéder service Login de API
login(model: any){
  return this.http.post(this.baseUrl+'login',model).pipe(
    map((response:any)=>{
      const user= response;
      //-> s'il ya un retour sauvgarder un objet token dans localStorage
      if(user){localStorage.setItem('token',user.token);}
    })
  )
}

register(model:any){
  return this.http.post(this.baseUrl+'register',model);
}

}
