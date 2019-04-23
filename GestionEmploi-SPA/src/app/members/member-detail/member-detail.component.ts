import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user:User
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private userService:UserService,private alertify:AlertifyService, private route:ActivatedRoute) { }

  ngOnInit() {
      // this.loadUser();
      this.route.data.subscribe(data=>{
        this.user=data['user']
      });

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

  
} 
