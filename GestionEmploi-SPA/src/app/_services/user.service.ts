import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginationResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl=environment.apiUrl+'users/';

constructor(private http:HttpClient) { }

//--> Méthode permet de retourner la liste des users sous forme tables User selon pagination
getUsers(page?,itemsPerPage?,userParams?):Observable<PaginationResult<User[]>>
{
  const paginationResult : PaginationResult<User[]> = new PaginationResult<User[]>();
  let params = new HttpParams();

  if(page != null && itemsPerPage != null){
    params = params.append('pagenumber', page);
    params = params.append('pageSize',itemsPerPage);

  }

  if(userParams != null ){
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge',userParams.maxAge);
    params = params.append('gender',userParams.gender);
    params = params.append('orderBy',userParams.orderBy);
  }



  return this.http.get<User[]>(this.baseUrl,{observe:'response',params}).pipe(
    map(response=>{
      paginationResult.result=response.body;
      if(response.headers.get('Pagination') != null){
        paginationResult.pagination =JSON.parse(response.headers.get('Pagination'))
      }
      return paginationResult;
    })
  );
  
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
