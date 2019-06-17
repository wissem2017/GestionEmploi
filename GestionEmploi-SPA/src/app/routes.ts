import { Routes } from '@angular/router'
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberdetailResolver } from './_resolvers/member-detail-resolver';
import { MemberlistResolver } from './_resolvers/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessageResolver } from './_resolvers/message.resolver';
import { PaymentComponent } from './payment/payment.component';
import { MessagesGuard } from './_guards/messages.guard';
import { ChargeGuard } from './_guards/charge.guard';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always'
        , canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,resolve:{
                users:MemberlistResolver
            } },
            { path: 'member/edit', component: MemberEditComponent ,resolve:{
                user:MemberEditResolver},canDeactivate:[PreventUnsavedChangesGuard]},
            { path: 'members/:id', component: MemberDetailComponent,resolve:{
                user:MemberdetailResolver
            } },
           
            { path: 'lists', component: ListsComponent,resolve:{
                users:ListsResolver
            } },
            { path: 'messages', component: MessagesComponent,canActivate:[MessagesGuard], resolve:{messages:MessageResolver} },

            { path: 'charge', component: PaymentComponent ,canActivate:[ChargeGuard]},

            { path: 'admin', component: AdminPanelComponent,data:{roles:['Admin','Moderator']} }


        ]
    },
   
    { path: '**', redirectTo: '', pathMatch: 'full' }
];