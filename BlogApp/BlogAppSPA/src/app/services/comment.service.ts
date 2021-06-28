import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Page } from '../models/page.model';
import { Comment } from '../models/comment.model';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  baseUrl = environment.url;

  update = new EventEmitter();

  constructor(private client: HttpClient) { }

  public getAll(postID: string, page: number = 1, search: string = ''): Promise<Page<Comment>> {
    const url = `${this.baseUrl}/api/comments?postid=${postID}&page=${page}&search=${search}`;
    return this.client.get<Page<Comment>>(url).pipe(catchError(this.handleError<Page<Comment>>('getAll'))).toPromise();
  }

  public async post(comment: Comment): Promise<unknown> {
    const url = `${this.baseUrl}/api/comments`;
    return this.client.post<Comment>(url, comment).pipe(tap(() => {
      this.update.emit();
    }), catchError(this.handleError<Comment>('post'))).toPromise();
  }

  public async put(comment: Comment): Promise<unknown> {
    const url = `${this.baseUrl}/api/comments/${comment.id}`;
    return this.client.put<Comment>(url, comment).pipe(tap(() => {
      this.update.emit();
    }), catchError(this.handleError<Comment>('put'))).toPromise();
  }

  public getReplies(commId: number, postId: string, pageIndex = 1): Promise<Page<Comment>> {
    const url = `${this.baseUrl}/api/comments/${commId}?postID=${postId}&page=${pageIndex}`;
    return this.client.get<Page<Comment>>(url).pipe(catchError(this.handleError<Page<Comment>>('getReplies'))).toPromise();
  }

  public async delete(commId: number): Promise<unknown> {
    const url = `${this.baseUrl}/api/comments/${commId}`;
    return await this.client.delete(url).pipe(tap(() => {
      this.update.emit();
    }), catchError(this.handleError<Page<Comment>>('getReplies'))).toPromise();
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);

      return of(result as T);
    };
  }
}
