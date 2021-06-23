import { Component, OnInit, ViewChild } from '@angular/core';
import { Page } from 'src/app/models/page.model';
import { Comment } from 'src/app/models/comment.model';
import { CommentService } from 'src/app/services/comment.service';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalComponent } from '../../shared/modal/modal.component';

@Component({
  selector: 'app-comments',
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.css'],
})
export class CommentsComponent implements OnInit {
  comments: Page<Comment>;

  isLoading = false;

  id: any;

  editID: any;

  replyID: any;

  repliesID: any;

  search = '';

  @ViewChild(ModalComponent) modal!: ModalComponent;

  constructor(private commsService: CommentService, private route: ActivatedRoute) {
    this.comments = {
      hasNextPage: false,
      hasPreviousPage: false,
      pageIndex: 0,
      items: [],
      pageSize: 0,
    };
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.route.paramMap.subscribe(async (params) => {
      this.id = params.get('id');
      await this.getAll(this.id);
    });
    this.commsService.update.subscribe(() => {
      this.getAll(this.id);
    });
  }

  async getAll(postID: string, page: number = 1) {
    this.comments = await this.commsService.getAll(postID, page, this.search);
    this.showEditor(null);
    this.showReplies(null);
    this.isLoading = false;
  }

  showEditor(id: number | null) {
    if (this.editID == id) {
      this.editID = null;
    } else {
      this.editID = id;
    }
  }

  showAddReplySection(id: number) {
    if (this.replyID == id) {
      this.replyID = null;
    } else {
      this.replyID = id;
    }

    this.repliesID = null;
  }

  showReplies(id: number | null) {
    if (this.repliesID == id) {
      this.repliesID = null;
    } else {
      this.repliesID = id;
    }

    this.replyID = null;
  }

  async showModal(id: number) {
    const response = await this.modal.open();

    if (response) {
      this.delete(id);
    }
  }

  async delete(id: number) {
    await this.commsService.delete(id);
    await this.getAll(this.id, this.comments.pageIndex);
  }
}
