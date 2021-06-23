import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Page } from '../models/page.model';
import { Post } from '../models/post.model';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  baseUrl = environment.url;

  constructor(private client: HttpClient) {}

  public getAll(page: number = 1, search: string = ''): Promise<Page<Post>> {
    const url = `${this.baseUrl}/api/posts?page=${page}&search=${search}`;
    return this.client.get<Page<Post>>(url).pipe(catchError(this.handleError<Page<Post>>('getAll'))).toPromise();
  }

  public get(id: string): Promise<Post> {
    const url = `${this.baseUrl}/api/posts/${id}`;
    return this.client.get<Post>(url).pipe(catchError(this.handleError<Post>('get(:id)'))).toPromise();
  }

  public post(post: Post): Promise<Post> {
    const url = `${this.baseUrl}/api/posts`;
    return this.client.post<Post>(url, post).pipe(catchError(this.handleError<Post>('post'))).toPromise();
  }

  public put(id: string, post: Post): Promise<unknown> {
    const url = `${this.baseUrl}/api/posts/${id}`;
    return this.client.put<Post>(url, post).pipe(catchError(this.handleError<Post>('put'))).toPromise();
  }

  public delete(id: string): Promise<any> {
    const url = `${this.baseUrl}/api/posts/${id}`;
    return this.client.delete(url).pipe(catchError(this.handleError('delete'))).toPromise();
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(error);

      return of(result as T);
    };
  }
}
