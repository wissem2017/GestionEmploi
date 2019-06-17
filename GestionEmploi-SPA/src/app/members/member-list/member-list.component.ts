import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginationResult } from 'src/app/_models/Pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users:User[]; //--> Variable ou on va sauvgarder liste des utilisateurs
  user:User= JSON.parse(localStorage.getItem('user'));
  genderList = [{value:'homme',display:'homme'},{value:'femme',display:'femme'}];
  userParams:any={};
  pagination:Pagination;
  search:boolean=false;

  constructor(private userService: UserService, private route: ActivatedRoute,
    private alertify: AlertifyService) { }

  ngOnInit() {
    // this.loadUsers();
    this.search=false;
    this.route.data.subscribe(
      data=>{
      this.users=data['users'].result;
      this.pagination=data['users'].pagination}
    );

    this.userParams.gender=this.user.geder==='homme'?'femme':'homme';
    this.userParams.minAge=18;
    this.userParams.maxAge=99;
    this.userParams.orderBy='lastActive';
  }

//--> retourner filter par défaut
  resetFilter(){
    this.userParams.gender=this.user.geder==='homme'?'femme':'homme';
    this.userParams.minAge=18;
    this.userParams.maxAge=99;
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

 
  loadUsers() {
    
    if (!this.search) {
      this.pagination.currentPage=1;
       }

    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage,this.userParams).subscribe((res: PaginationResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
      
    },
      error => this.alertify.error(error)
    );

  }

}
