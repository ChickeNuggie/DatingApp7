import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
//The ControlValueAccessor interface is used in Angular to create a bridge between Angular form controls (such as ngModel) and native HTML form elements.
//It defines methods that allow the form control to communicate with the native element, such as getting and setting the value of the input,
//and registering callbacks to be notified when the value changes.
//By implementing the ControlValueAccessor interface in TextInputComponent, you are creating a custom form control that can be used in Angular forms, 
// and you are defining the methods that Angular will use to interact with your custom control.

//It allow inputs in DOM to interact with text-input component if want to make code reuseable.
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type ='text';

  // self() decorator to ensure ng control is unique to that inputs that we are updating in the DOM
  // NgForm derieved from NgControl 
  constructor(@Self() public ngControl: NgControl)  { 
  // pass ngcontrol and set value accessor to textinputcomponent which implement controlvalueaccessor that;
  //  allows us to write values, register input that has been change/touchd.
    this.ngControl.valueAccessor = this; // this = text input component class
  }
  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }


  get control(): FormControl {
    return this.ngControl.control as FormControl // effectively cast control into form control to get aroud typescript error.
  }



}
