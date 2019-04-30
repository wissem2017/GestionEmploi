import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { frLocale } from 'ngx-bootstrap/locale';
import { User } from '../_models/user';
import { Router } from '@angular/router';
defineLocale('fr', frLocale); 



@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output()cancelRegister = new EventEmitter(); //--> Permet d'envoyé des donnée vers page père (home)
  user: User;
  registerForm:FormGroup;
  bsConfig:Partial<BsDatepickerConfig>;
  locale='fr';

  constructor(private router: Router,private authService:AuthService, private alertify:AlertifyService, private fb: FormBuilder,private localeService: BsLocaleService) {
    this.localeService.use(this.locale);
   }

  ngOnInit() {
    //-->Conféguer calendier date
    this.bsConfig={
      containerClass:'theme-dark-blue',
      showWeekNumbers:false
    }
    this.createRegisterForm();
  }

  //--> Méthode permet de créer registerForm
  createRegisterForm(){
    this.registerForm=this.fb.group({
      gender:['Home'],
      username:['',Validators.required],
      knowAs:['',Validators.required],
      dateOfBirth:[null,Validators.required],
      city:['',Validators.required],
      country:['',Validators.required],
      password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword:['',Validators.required]

    },{validator:this.passwordMatchValidator})
  }

  register(){
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        () => { this.alertify.success('Abonné avec succès') },
        error => { this.alertify.error(error) },
        () => {
          this.authService.login(this.user).subscribe(
            () => {
              this.router.navigate(['/members']);
            }

          )
        }
      )
    }
  }

  //-->Méthode permet de vérifier si le PWD et confirme PWD sont égaux
  passwordMatchValidator(form:FormGroup){
    return form.get('password').value===form.get('confirmPassword').value ? null : {'mismatch':true};
  }

  cancel(){
   
    //--> lorsque qu'on clique sur "Pas maintenant" envoyé false pour le page père (home)
    this.cancelRegister.emit(false);
  }

}
