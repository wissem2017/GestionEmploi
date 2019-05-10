import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user:User; //--> Indique que ce componant ne fontionne que si je luis donne user

  constructor(private authService:AuthService, private userService:UserService,private alertify:AlertifyService) { }

  ngOnInit() {
  }

  sendLike(id:number){
    this.userService.sendLike(this.authService.decodedToken.nameid,id).subscribe(
      ()=>{this.alertify.success('Tu as admirais '+this.user.username);},
      error=>{this.alertify.error(error)}
    );
  }

}
