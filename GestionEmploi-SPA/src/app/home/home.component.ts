import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode:boolean=false;
 
  constructor(private http:HttpClient) { }

  ngOnInit() {
    
  }

  registerToggle(){
    this.registerMode=true;
  }

   //--> méthode sera exécuter lors le page fis (register) clique sur "pas maintenant" pour modifier 
  //  le contenu de registerMode = false
  cancelRegister(mode:boolean){
    this.registerMode=mode;
  }

}
