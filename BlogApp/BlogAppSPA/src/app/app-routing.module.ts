import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostCreateComponent } from './modules/posts/create-post/create-post.component';
import { PostDetailsComponent } from './modules/posts/details-post/details-post.component';
import { PostEditComponent } from './modules/posts/edit-post/edit-post.component';
import { PostsComponent } from './modules/posts/posts/posts.component';
import { PageNotFoundComponent } from './modules/shared/page-not-found/page-not-found.component';
import { UsersComponent } from './modules/users/users/users.component';

const routes: Routes = [
  { path: 'users', component: UsersComponent },
  { path: '', component: PostsComponent },
  { path: 'details/:id', component: PostDetailsComponent },
  { path: 'create', component: PostCreateComponent },
  { path: 'edit/:id', component: PostEditComponent },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
