import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl=environment.apiUrl+'users/';

constructor(private http:HttpClient) { }

//--> Méthode permet de retourner la liste des users sous forme tables User
getUsers():Observable<User[]>{
  return this.http.get<User[]>(this.baseUrl);
}

//--> Méthode permet de retourner user selon id
getUser(id):Observable<User>{
  return this.http.get<User>(this.baseUrl+id);
}


}
