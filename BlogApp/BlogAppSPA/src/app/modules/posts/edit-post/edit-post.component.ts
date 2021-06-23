import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Post } from 'src/app/models/post.model';
import { ImageService } from 'src/app/services/image.service';
import { PostService } from 'src/app/services/post.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-edit-post',
  templateUrl: './edit-post.component.html',
  styleUrls: ['./edit-post.component.css'],
})
export class PostEditComponent implements OnInit {
  id: any = '';

  isLoading = false;

  post: Post;

  imgURL: any;

  public message: string = '';

  public image!: File;

  baseUrl = environment.url;

  constructor(private route: ActivatedRoute, private postService: PostService, private router: Router, private imageService: ImageService) {
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
    if (this.post.imageURL != null) {
      this.imgURL = this.baseUrl + this.post.imageURL;
    }
    this.isLoading = false;
  }

  async onSubmit() {
    const response = await this.postService.put(this.id, this.post);

    if (this.image) {
      await this.uploadImage(this.post.id);
    }

    await this.router.navigate(['/details/', this.post.id]);
  }

  async uploadImage(id: number) {
    const formData: any = new FormData();
    formData.append('File', this.image);
    formData.append('PostID', id);
    await this.imageService.put(id, formData);
  }

  preview(file: FileList | null) {
    this.message = '';
    if (file == null) {
      return;
    }

    if (file.length === 0) {
      this.imgURL = undefined;
      return;
    }

    const mimeType = file[0].type;
    if (mimeType.match(/image\/*/) == null) {
      this.message = 'Only images are supported.';
      return;
    }

    const reader = new FileReader();
    this.image = file[0];
    reader.readAsDataURL(file[0]);
    reader.onload = (_event) => {
      this.imgURL = reader.result;
    };
  }
}
