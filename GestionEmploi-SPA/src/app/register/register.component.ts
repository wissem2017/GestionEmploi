import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output()cancelRegister = new EventEmitter(); //--> Permet d'envoyé des donnée vers page père (home)


  model:any ={};

  constructor(private authService:AuthService, private alertify:AlertifyService) { }

  ngOnInit() {
  }

  register(){
  this.authService.register(this.model).subscribe(
    ()=>{this.alertify.success('Abonné avec succès')},
    error=>{this.alertify.error(error)}
  );
  }

  cancel(){
   
    //--> lorsque qu'on clique sur "Pas maintenant" envoyé false pour le page père (home)
    this.cancelRegister.emit(false);
  }

}
