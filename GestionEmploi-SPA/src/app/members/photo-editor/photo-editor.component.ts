import { Component, OnInit, Input, Output, EventEmitter, } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})

export class PhotoEditorComponent implements OnInit {

  @Input() photos:Photo[] //--> pour inporter des données de pére vers fils
  @Output() getMemberPhotoChange=new EventEmitter<string>();//--> pour faire la laison entre fis et pére
  uploader:FileUploader ;
  hasBaseDropZoneOver= false;
  baseUrl=environment.apiUrl;
  currentMain:Photo;
  user:User;

    
  constructor(private authService:AuthService,private route:ActivatedRoute, private userService:UserService, private alertify:AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
    // this.route.data.subscribe(data=>{
    //   this.user=data['user']
    // })
  }

  fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(){
    //-->Déterminer la caractéristique de fichier télécharger
    this.uploader=new FileUploader(
      {
        url:this.baseUrl+'users/'+this.authService.decodedToken.nameid+'/photos',
        authToken:'Bearer '+localStorage.getItem('token'),
        isHTML5:true,
        allowedFileType:['image'],
        removeAfterUpload:true,
        autoUpload:false,
        maxFileSize:10*1024*1024
      }
    );

    this.uploader.onAfterAddingFile=(file)=>{file.withCredentials=false;};
   //--> Aprés téléchargement de photos
    this.uploader.onSuccessItem=(item,Response,status,headers)=>{
      if(Response){
        const res:Photo = JSON.parse(Response);
        const photo ={
          id:res.id,
          url:res.url,
          dateAdded:res.dateAdded,
          isMain:res.isMain,
          isApproved:res.isApproved
        };
        this.photos.push(photo); //--> Ajouter photo 
        if(photo.isMain){
          this.authService.changeMemberPhoto(photo.url);
          this.authService.currentUser.photoUrl=photo.url;
          localStorage.setItem('user',JSON.stringify(this.authService.currentUser));
        }
      }
    }
  }


  //---> Méthode pour modifier photo principale
  setMainPhoto(photo:Photo){
    this.userService.setMainPhoto(this.authService.decodedToken.nameid,photo.id).subscribe(
      ()=>{this.currentMain=this.photos.filter(p=>p.isMain===true)[0];
        this.currentMain.isMain=false;
        photo.isMain=true;
        // this.getMemberPhotoChange.emit(photo.url);
        
        // this.user.photoUrl=photo.url;
        this.authService.changeMemberPhoto(photo.url);
        this.authService.currentUser.photoUrl=photo.url;
        localStorage.setItem('user',JSON.stringify(this.authService.currentUser));

      },
      ()=>{this.alertify.error('Il un problème pour photo principale');}
       )
  }

  //--> Méthode permet de supprimer photo
  delete(id:number){
    this.alertify.confirm("Voulez vous supprimer cette photo",()=>{
      this.userService.deletePhoto(this.authService.decodedToken.nameid,id).subscribe(
        ()=>{
          this.photos.splice(this.photos.findIndex(p=>p.id===id),1);
          this.alertify.success("Photo supprimée avec succès");
        },
        erroe=>{this.alertify.error("Une erreur est survenue lors de la suppression de l'image.");}
      );
    });
  }

}
