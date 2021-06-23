import {
  Component, EventEmitter, OnInit, Output,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Comment } from 'src/app/models/comment.model';
import { CommentService } from 'src/app/services/comment.service';

@Component({
  selector: 'app-create-comment',
  templateUrl: './create-comment.component.html',
  styleUrls: ['./create-comment.component.css'],
})
export class CreateCommentComponent implements OnInit {
  isLoading = false;

  id: any;

  model: Comment;

  @Output() addedNewCommentEvent = new EventEmitter();

  constructor(private route: ActivatedRoute, private commService: CommentService) {
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
    this.isLoading = true;
    this.route.paramMap.subscribe((params) => {
      this.id = params.get('id');
    });
  }

  async onSubmit() {
    this.model.postID = this.id;
    await this.commService.post(this.model);
    this.model.content = '';
  }
}
