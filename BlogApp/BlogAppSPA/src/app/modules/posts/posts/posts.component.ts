import { Component, Injectable, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Page } from 'src/app/models/page.model';
import { Post } from 'src/app/models/post.model';
import { PostService } from 'src/app/services/post.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.css'],
})

@Injectable({ providedIn: 'root' })

export class PostsComponent implements OnInit {
  posts: Page<Post>;

  baseUrl = environment.url;

  isLoading = false;

  search ='';

  constructor(private postService : PostService, private activatedRoute : ActivatedRoute) {
    this.posts = {
      hasNextPage: false,
      hasPreviousPage: false,
      pageIndex: 0,
      items: [],
      pageSize: 0,
    };
  }

  async ngOnInit() {
    this.isLoading = true;
    this.getAll();
  }

  async getAll(page: number = 1): Promise<void> {
    this.posts =  await this.postService.getAll(page, this.search);
    this.isLoading = false;
  }
}
