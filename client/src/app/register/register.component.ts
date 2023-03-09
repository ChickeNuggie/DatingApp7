import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();// From child component to parent component
  model: any = {} // form
  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.register(this.model).subscribe({
      //what to do next with the response obtained from form?
      next: () => {
        this.cancel(); // close reghister form
      },
      error: error => {
        this.toastr.error(error.error);
        console.log(error);
      }
    })
  }

  cancel() {
    this.cancelRegister.emit(false); //turn off register mode in home components upon cancelling
  }
}
