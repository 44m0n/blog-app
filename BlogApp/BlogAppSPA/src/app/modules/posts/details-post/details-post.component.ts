import {
  Component, ElementRef, OnInit, ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Post } from 'src/app/models/post.model';
import { PostService } from 'src/app/services/post.service';
import { environment } from 'src/environments/environment';
import { ModalComponent } from '../../shared/modal/modal.component';

@Component({
  selector: 'app-details-post',
  templateUrl: './details-post.component.html',
  styleUrls: ['./details-post.component.css'],
})
export class PostDetailsComponent implements OnInit {
  id: any = '';

  isLoading = false;

  post: Post;

  baseUrl = environment.url;

  @ViewChild(ModalComponent) modal!: ModalComponent;

  constructor(private route: ActivatedRoute, private postService: PostService, private router: Router) {
    this.post = {
      id: 0,
      title: '',
      content: '',
      createdAt: '',
      modifiedAt: '',
      imageURL: '',
      userID: '',
      owner: '',
    };
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.route.paramMap.subscribe((params) => {
      this.id = params.get('id');
      this.get(this.id);
    });
  }

  async get(id: string): Promise<void> {
    this.post = await this.postService.get(id);
    this.isLoading = false;
  }

  async delete(): Promise<void> {
    await this.postService.delete(this.id);
    await this.router.navigate(['']);
  }

  async showModal() {
    const response = await this.modal.open();

    if (response) {
      this.delete();
    }
  }
}
