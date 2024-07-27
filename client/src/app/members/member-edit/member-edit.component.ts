import { Component, HostListener, OnInit, ViewChild, inject } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/Member';
import { AccountService } from '../../_services/account.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.loadMember()
    
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;
    console.log(user.username);

    this.memberService.getMember(user.username).subscribe({
      next: response => this.member = response,
      error: error => console.log(error)
    })
  }
  updateMember() {
    this.memberService.updateUser(this.editForm?.value).subscribe({
      next: () => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member);
      }

    });

  }
}
