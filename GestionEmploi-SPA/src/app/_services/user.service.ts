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

//--> Méthode permet de MAJ User
udpateUser(id:number, user:User){
  return this.http.put(this.baseUrl+id,user);
}

//-->Permet de modifier photo principale
setMainPhoto(userId:number, id:number){
  return this.http.post(this.baseUrl+ userId+'/photos/'+id+'/setMain',{});
}

//--> Permet de supprimer photo user
deletePhoto(userId:number, id:number){
  return this.http.delete(this.baseUrl+userId+'/photos/'+id);
}



}
