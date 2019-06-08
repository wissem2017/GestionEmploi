import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginationResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl=environment.apiUrl+'users/';

constructor(private http:HttpClient) { }

//--> Méthode permet de retourner la liste des users sous forme tables User selon pagination
getUsers(page?,itemsPerPage?,userParams?,likeParam?):Observable<PaginationResult<User[]>>
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

  if(likeParam==='Likers')
  {
    params = params.append('likers', 'true');
  }
  if(likeParam==='Likees')
  {
    params = params.append('likees', 'true');
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

//--> Ajouter Like
sendLike(id:number,recipientId:number)
{
  return this.http.post(this.baseUrl+id+'/like/'+recipientId,{});
}

//-->méthode permet de retourner la liste des messages
getMessages(id:number,page?,itemsPerPage?,messageType?)
{
  const paginationResult : PaginationResult<Message[]> = new PaginationResult<Message[]>();

  //-->Créer la liste des paramètre envoyé à API
  let params=new HttpParams();
  params=params.append('MessageType',messageType);
  if(page != null && itemsPerPage != null){
    params = params.append('pagenumber', page);
    params = params.append('pageSize',itemsPerPage);
  }

  //--> faire la connexion avec API
  return this.http.get<Message[]>(this.baseUrl+id+'/messages',{observe:'response',params}).pipe(
    map(response=>{
      paginationResult.result=response.body;
      if(response.headers.get('Pagination')!==null){
        paginationResult.pagination=JSON.parse(response.headers.get('Pagination'))
      }
      return paginationResult;
    })
  );
}

getConversation(id:number,recipientId:number)
{
  return this.http.get<Message[]>(this.baseUrl+id+'/messages/chat/'+recipientId);
}

sendMessage(id:number,message:Message)
{
  return this.http.post(this.baseUrl+id+'/messages',message);
}

getUnreadCount(userId)
{
  return this.http.get(this.baseUrl+userId+'/messages/count');
}

markAsRead(userId:number, messageId:number)
{
  return this.http.post(this.baseUrl+userId+'/messages/read/'+messageId,{}).subscribe();
}

//Métode pour accéser au service API pour supprimer Message
deleteMessage(id:number,userId:number)
{
  return this.http.post(this.baseUrl+userId+'/messages/'+id,{});
}

}
