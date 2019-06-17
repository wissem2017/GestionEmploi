import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  constructor(private adminService: AdminService , private alertify:AlertifyService) { }
  photos: any[];
  ngOnInit() {
    this.getPhotosForApproval();
  }
   getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe((photos:any[]) => {
      this.photos = photos;
    }, () => {
      this.alertify.error('Il y a un problème affichage des images');
    });
  }

  approvePhoto(photoId) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
    }, () => {
      this.alertify.error('Il y a un problème acceptation de image');
    });
  }

  rejectPhoto(photoId) {
    this.adminService.rejectPhoto(photoId).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
    }, () => {
      this.alertify.error('Il y a un problème avec la désapprobation de la photo');
    });
  }
}
