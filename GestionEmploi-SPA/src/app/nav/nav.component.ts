import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  //--> Création objet pour reçoi Login et PWD
  model :any = {}; 

  constructor(public authService:AuthService, private alertify:AlertifyService, private router:Router) { }

  ngOnInit() {
  }

  //--> Méthode permet de retouner les valeur de Login et PWD dans le console
  login(){
    this.authService.login(this.model).subscribe(
      next=>{this.alertify.success('Entrer Avec Succsse')},
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
    this.alertify.message('Déconnexion avec success');
    this.router.navigate(['/home']); //--> Retourner vers page Home
  }

}
