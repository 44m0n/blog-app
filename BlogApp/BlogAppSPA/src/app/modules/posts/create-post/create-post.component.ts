import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Post } from 'src/app/models/post.model';
import { ImageService } from 'src/app/services/image.service';
import { PostService } from 'src/app/services/post.service';

@Component({
  selector: 'app-create-post',
  templateUrl: './create-post.component.html',
  styleUrls: ['./create-post.component.css'],
})
export class PostCreateComponent implements OnInit {
  submitted = false;

  model: Post;

  public image!: File;

  imgURL: any;

  public message: string = '';

  constructor(private postService: PostService, private imageService: ImageService, private router: Router) {
    this.model = {
      title: '',
      id: 0,
      content: '',
      createdAt: '',
      modifiedAt: '',
      imageURL: null,
      userID: '',
      owner: '',
    };
  }

  ngOnInit(): void {
  }

  async onSubmit() {
    const response = await this.postService.post(this.model);

    if (this.image) {
      await this.uploadImage(response.id);
    }

    await this.router.navigate(['']);
  }

  async uploadImage(id: number) {
    const formData: any = new FormData();
    formData.append('File', this.image);
    formData.append('PostID', id);

    this.imageService.put(id, formData);
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
