import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  //--> Création objet pour reçoi Login et PWD
  model :any = {}; 

  constructor(private authService:AuthService) { }

  ngOnInit() {
  }

  //--> Méthode permet de retouner les valeur de Login et PWD dans le console
  login(){
    this.authService.login(this.model).subscribe(
      next=>{console.log('Entrer Avec Succsse')},
      error=>{console.log('Erreur Entrée')}
    )
  }

  loggedIn(){
    //--> On récupére la valeur de token s'il existe affiche Bienvenu 
    const token=localStorage.getItem('token');
    //si token eixte retourn true sinon return false
    return !!token; 
  }

  loggedOut(){
    //--> Supprimer token lors de déconnexion
    localStorage.removeItem('token');
    console.log('Dconnexion avec success');
  }

}
