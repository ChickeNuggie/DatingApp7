import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { __values } from 'tslib';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();// From child component to parent component
  // component based that coreate sna control form via component than html, 
  // easier to use and control the validation and testing in reactive form submission than in html.
  registerForm: FormGroup = new FormGroup({}); 
  maxDate: Date = new Date(); // set max date for age requirement
  validationErrors: string[] | undefined;

  // formbuild = service from reactive forms
  // redirect user after successful login using router.
  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  initializeForm() { 
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]], //match with password control form
    });
    // Ensure password is invalid when it changed when comparing with confirmed password.
    //access password control form, check for value changes and subscribe to the observable response.
    //next step, access confirm password control form to be compared against password and update value and validiity.
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  //check if control form values is equal to another control password, if do match return null else create object and set to true.
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  register() { 
     const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    // override the value inside registerform value for the date of birth by
    // using the spread operator (...) to copy all the properties of an existing object, this.registerForm.value and 
    // adds a new property called dateOfBirth to the new object, with the value of the dob variable.
    // This new object can be used to send the form data to a server or perform other operations that require the complete form data with the date of birth included.
     const values = {...this.registerForm.value, dateOfBirth: dob} 

    this.accountService.register(values).subscribe({ // get the values from customized registerForm
      //what to do next with the response obtained from form?
      next: () => {
        this.router.navigateByUrl('/members') // // redirect user to members page after successful login using router.
      },
      error: error => {
        this.validationErrors = error
      }
    })
  }

  cancel() {
    this.cancelRegister.emit(false); //turn off register mode in home components upon cancelling
  }

  // customize to standardized date of birth regardless of timezone.
  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10);

  }
}
