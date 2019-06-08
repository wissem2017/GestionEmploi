import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs:TabsetComponent;//pour faire la laison avec TabSet
  user:User
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  created:string;
  age:string;
  showIntro:boolean=true;
  showLook:boolean=true;
  options={weekday:'long', year:'numeric', month:'long',day:'numeric'};

  constructor(private userService:UserService,private authservice:AuthService,private alertify:AlertifyService, private route:ActivatedRoute) { }

  ngOnInit() {
      // this.loadUser();
      this.route.data.subscribe(data=>{
        this.user=data['user']
      });

      this.route.queryParams.subscribe(
        params=>{
          const selectedTab=params['tab'];
          this.memberTabs.tabs[selectedTab>0?selectedTab:0].active=true;
        }
      )

      //--> Caractérestique de l'image 
      this.galleryOptions=[{
        width:'500px',
        height:'500px',
        imagePercent:100,
        thumbnailsColumns:4,
        imageAnimation:NgxGalleryAnimation.Slide,
        preview:false //--> Pour que l'image reste dans la même zone
      }]

      //--> Caractéristique de galerie image
      this.galleryImages=this.getImages();
      this.created=new Date(this.user.created).toLocaleString('fr-fr',this.options);
      this.showIntro=true;
      this.showLook=true;
  }

  selectTab(tabId:number){
    this.memberTabs.tabs[tabId].active=true;
  }

//--> Méthode permet de retourner la liste des image users
  getImages(){
    const imageUrls=[];
    for(let i=0; i<this.user.photos.length; i++){
      //--> utilser "push" pour ajouter à la fin de table
      imageUrls.push({
        small:this.user.photos[i].url,
        medium:this.user.photos[i].url,
        big:this.user.photos[i].url,
      })
    };
    return imageUrls;
  }

  deselect(){
    this.authservice.hubConnection.stop();
  }
  

  
} 
