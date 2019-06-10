import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { UserService } from '../_services/user.service';
import { HubConnection , HubConnectionBuilder} from '@aspnet/signalr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  //--> Création objet pour reçoi Login et PWD
  model :any = {}; 
  photoUrl:string;
  count:string;
  hubConnection:HubConnection;

  constructor(public authService:AuthService,private  userService:UserService,private alertify:AlertifyService, private router:Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(
      photoUrl=>this.photoUrl=photoUrl);

      if(this.loggedIn){
        this.userService.getUnreadCount(this.authService.decodedToken.nameid).subscribe(
          res=>{this.authService.unreadCount.next(res.toString());
          this.authService.latestUnreadCount.subscribe(res=>{this.count=res});
          }
         );
        this.getPaymentForUser();
      }
     
     this.hubConnection = new HubConnectionBuilder().withUrl("http://localhost:5000/chat").build();
    this.hubConnection.start();
    this.hubConnection.on('count', () => {
      setTimeout(() => {
            this.userService.getUnreadCount(this.authService.decodedToken.nameid).subscribe(res=>{
              this.authService.unreadCount.next(res.toString());
              this.authService.latestUnreadCount.subscribe(res=>{this.count=res;});
            });
          }, 0);
    });

  }

  //--> Méthode permet de retouner les valeur de Login et PWD dans le console
  login(){
    this.authService.login(this.model).subscribe(
      next=>{this.alertify.success('Entrer Avec Succsse');
       this.userService.getUnreadCount(this.authService.decodedToken.nameid).subscribe(res=>{
        this.authService.unreadCount.next(res.toString());
        this.authService.latestUnreadCount.subscribe(res=>{this.count=res;});
        this.getPaymentForUser();
             });	
      },
      error=>{this.alertify.error(error)},
      ()=>{this.router.navigate(['/members']);}
    )
  }

  loggedIn(){
    
    return this.authService.loggedIn(); 
  }

  loggedOut(){
    //--> Supprimer token lors de déconnexion
    localStorage.removeItem('token');
    this.authService.decodedToken=null;
    
    this.authService.paid=false;
    
    localStorage.removeItem('user');
    this.authService.currentUser=null
    
    this.alertify.message('Déconnexion avec success');
    this.router.navigate(['/home']); //--> Retourner vers page Home
  }

  //Méthode permet de vérifier si user à payer ses frais
  getPaymentForUser()
  {
    this.userService.getPaymentForUser(this.authService.currentUser.id).subscribe(
      res=>{
        if(res !=null)
          this.authService.paid=true;
        else
        this.authService.paid=false;
      }
    )
  }

}
