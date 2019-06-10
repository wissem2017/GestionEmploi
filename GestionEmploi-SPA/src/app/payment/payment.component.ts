import {
  Component,
  AfterViewInit,
  OnDestroy,//permet de detrouire l'objet carte
  ViewChild,
  ElementRef,
  ChangeDetectorRef,OnInit
} from '@angular/core';

import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';

import {Location} from '@angular/common';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})

export class PaymentComponent implements OnInit,OnDestroy,AfterViewInit {
  @ViewChild('cardInfo') cardInfo: ElementRef;

  card: any ;
  cardHandler = this.onChange.bind(this);
  error: string;
  successPaid:boolean=false;
  loader:boolean=false;

  constructor(private cd: ChangeDetectorRef, private userService:UserService,private authService:AuthService , private location:Location,private route:ActivatedRoute) {}

  ngAfterViewInit() {
    
    const style = {
      base: {
        fontFamily: 'monospace',
        fontSmoothing: 'antialiased',
        fontSize: '21px',
        '::placeholder': {
          color: 'purple'
        }
      }
    };
  
    this.card = elements.create('card', {hidePostalCode: true ,style: style });
    this.card.mount(this.cardInfo.nativeElement);
    // this.card.mount('#card-info');
      
    this.card.addEventListener('change', this.cardHandler);
   
  }

  ngOnDestroy() {
    this.card.removeEventListener('change', this.cardHandler);
    this.card.destroy();
  }

  onChange({ error }) {
    if (error) {
      this.loader=false;
      if(error.message==='Your card number is incomplete.')
      this.error="Votre numéro de carte est incorrect"
      if(error.message==="Your card's expiration date is incomplete.")
      this.error="La date d'expiration de la carte est incorrecte"
      if(error.message==="Your card's security code is incomplete.")
      this.error="Le code de sécurité est incorrect"
     
     
    } else {
      this.error = null;
    }
    this.cd.detectChanges();
  }

  
  ngOnInit() {
  
  }
  async onSubmit() {
   
    const { token, error } = await stripe.createToken(this.card);
    
    if (error) {
      console.log('Il y a eu une erreur de paiement', error);
    } else {
      
      this.userService.charge(this.authService.currentUser.id,token.id.toString()).subscribe(
        res=>{
         this.successPaid=res['isPaid'];
         setTimeout(() => {
          this.authService.paid=res['isPaid'];
         }, 2000);
        }
      )
        
    }
  }
  backClicked() {
    this.location.back();
  }
  load(){
    if(this.error===null)
    this.loader=true;
    else this.loader=false;
  }
  
}