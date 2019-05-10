import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms"; //--> Permet de faire la laison entre page HTML et component
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';



import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvidor } from './_services/error.intercetor';
import { AlertifyService } from './_services/alertify.service';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import {TimeAgoPipe} from 'time-ago-pipe';


import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { UserService } from './_services/user.service';
import { MemberdetailResolver } from './_resolvers/member-detail-resolver';
import { MemberlistResolver } from './_resolvers/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { ListsResolver } from './_resolvers/lists.resolver';





/*Méthode permet de retourner token*/
export function tokenGetter(){
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      ListsComponent,
      MessagesComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      TimeAgoPipe,
      PhotoEditorComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      FileUploadModule,
      BsDropdownModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      TabsModule.forRoot(),
      BsDatepickerModule.forRoot(),
      ButtonsModule,
      NgxGalleryModule,
      PaginationModule.forRoot(),
      JwtModule.forRoot({
         config:{
            tokenGetter:tokenGetter,
            whitelistedDomains:['localhost:5000'],
            blacklistedRoutes:['localhost:5000/api/auth']
         }
      })
   ],
   providers: [
      AuthService,
      ErrorInterceptorProvidor,
      AlertifyService,
      AuthGuard,
      UserService,
      MemberdetailResolver,
      MemberEditResolver,
      PreventUnsavedChangesGuard,
      MemberlistResolver,
      ListsResolver
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
