import {
  Component, EventEmitter, Input, OnInit, Output,
} from '@angular/core';
import { CommentService } from 'src/app/services/comment.service';
import { Comment } from 'src/app/models/comment.model';

@Component({
  selector: 'app-edit-comment',
  templateUrl: './edit-comment.component.html',
  styleUrls: ['./edit-comment.component.css'],
})
export class EditCommentComponent implements OnInit {
  @Input()
  model!: Comment;

  updatedContent!: string;

  @Output() editedEvent = new EventEmitter<any>();

  constructor(private commService: CommentService) {
  }

  ngOnInit(): void {
    this.updatedContent = this.model.content;
  }

  async onSubmit() {
    this.model.content = this.updatedContent;
    await this.commService.put(this.model);
  }
}
