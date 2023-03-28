import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
// creating a custom form control that can be used in Angular forms, and you are defining the methods that Angular will use to interact with your custom control.
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() maxDate: Date | undefined;
  bsConfig: Partial<BsDatepickerModule> | undefined; // choose which properties option to pass through in the configuration

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this; // set value to datepicker component
    this.bsConfig= {
      containerClass: 'theme-red',
      dateInputFormat: 'DD MMM YYYY'
    }
   }
  
  
  writeValue(obj: any): void {

  }
  registerOnChange(fn: any): void {

  }
  registerOnTouched(fn: any): void {

  }

  get control(): FormControl {
     return this.ngControl.control as FormControl
  }

}
