import { Component, Input, OnInit } from '@angular/core';
import { CommentService } from 'src/app/services/comment.service';
import { Comment } from 'src/app/models/comment.model';

@Component({
  selector: 'app-create-reply',
  templateUrl: './create-reply.component.html',
  styleUrls: ['./create-reply.component.css'],
})
export class CreateReplyComponent implements OnInit {
  model: Comment;

  @Input()
  postID: any;

  @Input()
  parrentID: any;

  constructor(private commService: CommentService) {
    this.model = {
      id: 0,
      userID: '',
      postID: -1,
      content: '',
      date: new Date(),
      parentID: null,
      userFullName: '',
      repliesCount: 0,
    };
  }

  ngOnInit(): void {
  }

  async onSubmit() {
    this.model.postID = this.postID;
    this.model.parentID = this.parrentID;
    await this.commService.post(this.model);
    this.model.content = '';
  }
}
