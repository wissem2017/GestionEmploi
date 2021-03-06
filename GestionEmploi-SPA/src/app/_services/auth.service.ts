import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from "rxjs/operators";
import { JwtHelperService } from "@auth0/angular-jwt";
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  jwtHelper= new JwtHelperService();
  
  _lang:string='fr';
  dir:string='ltr';



  decodedToken:any;
  
  baseUrl=environment.apiUrl+'auth/'; //--> URL Controleur API auth
  
  currentUser:User;

  paid:boolean=false;

  photoUrl=new BehaviorSubject<string>('../../assets/user.png');//--> Photo par défaux
  currentPhotoUrl=this.photoUrl.asObservable();

  language= new BehaviorSubject<string>('fr');
  lang=this.language.asObservable();

  unreadCount=new BehaviorSubject<string>('');
  latestUnreadCount =this.unreadCount.asObservable();

  hubConnection:HubConnection = new HubConnectionBuilder().withUrl("http://localhost:5000/chat").build();


  constructor(private http:HttpClient) {
    this.lang.subscribe(
      lang=>{
        if (lang=='fr'){
          this.dir='ltr';
          this._lang='fr';
        }else if(lang=='en')
        {
          this.dir='ltr';
          this._lang='en';
        }else{
          this.dir='rtl';
          this._lang='ar';
        }
      }
    );
   }

  //--> Méthode permet de faire mise à jour photo de navbar
  changeMemberPhoto(newPhotoUrl:string){
    this.photoUrl.next(newPhotoUrl);
  }

//--> Méthode qui permet d'accéder service Login de API
login(model: any){
  return this.http.post(this.baseUrl+'login',model).pipe(
    map((response:any)=>{
      const user= response;
      //-> s'il ya un retour sauvgarder un objet token dans localStorage
      if(user){
        localStorage.setItem('token',user.token);
        localStorage.setItem('user',JSON.stringify(user.user));

        this.decodedToken=this.jwtHelper.decodeToken(user.token);
        this.currentUser=user.user;
        this.changeMemberPhoto(this.currentUser.photoUrl);
      }
    })
  )
}

register(user:User){
  return this.http.post(this.baseUrl+'register',user);
}

loggedIn(){
  try {
    const token=localStorage.getItem('token');
  return ! this.jwtHelper.isTokenExpired(token);
  } catch {
    return false;
  } 
}

roleMatch(AllowedRoles:Array<string>):boolean{
  let isMatch=false;
  const userRoles=this.decodedToken.role as Array<string>;
  AllowedRoles.forEach(element =>{
    if(userRoles.includes(element)){
      isMatch=true;
      return;
    }
  });
  return isMatch;
}

}
