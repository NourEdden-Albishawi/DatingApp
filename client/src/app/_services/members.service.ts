import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { of, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Member } from '../_models/Member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users').subscribe({
      next: members => this.members.set(members)
    });
  }


  getMember(username: string) {
    const member = this.members().find(user => user.userName == username);
    if (member != undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateUser(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      tap(() => {
        this.members.update(members => members.map(m => m.userName == member.userName ? member : m))
      })
    );
  }

}
