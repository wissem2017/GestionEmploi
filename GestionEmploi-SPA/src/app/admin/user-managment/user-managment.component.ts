import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-managment',
  templateUrl: './user-managment.component.html',
  styleUrls: ['./user-managment.component.css']
})
export class UserManagmentComponent implements OnInit {
 users:User[];
 bsModalRef: BsModalRef;
  constructor(private adminService:AdminService,private alertify:AlertifyService,private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.adminService.getUsersWithRoles().subscribe(
      (users:User[])=>{
        this.users=users;
      },
      error=>{
        this.alertify.error('Un problème est survenu lors de la récupération des utilisateurs');
      }
    )
  }

  //-->Créer Modal
  editRolesModal(user:User){
    const initialState = {
     user,
     roles : this.getRolesArray(user)
    };

    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});

    this.bsModalRef.content.updateSelectedRoles.subscribe((values)=>{
      const rolesToUpdate = {
        roleNames :[...values.filter(el=>el.checked===true).map(el=>el.value)]
      };
     if(rolesToUpdate){
       this.adminService.updateUserRoles(user,rolesToUpdate).subscribe(
         ()=>{
           user.roles = [...rolesToUpdate.roleNames];
         },
         error=>this.alertify.error(error)
       );
     }
    })
    
  }

  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles as any[];
    const availableRoles: any[] = [
      {name: 'Administrateur', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'},
      {name: 'Abonné', value: 'VIP'},
    ];

    availableRoles.forEach(aRole=>{
      let isMatch =false;
      userRoles.forEach(uRole=>{
        if(aRole.value===uRole){
          isMatch=true;
          aRole.checked = true;
          roles.push(aRole);
          return;
         }
      })
      if(!isMatch){
        aRole.checked=false;
        roles.push(aRole);
      }
    })
    return roles;
  }

}
