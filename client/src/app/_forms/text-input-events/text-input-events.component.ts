import { Component, Input, Self } from '@angular/core';
import { NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input-events',
  templateUrl: './text-input-events.component.html',
  styleUrls: ['./text-input-events.component.css']
})
export class TextInputEventsComponent {
  @Input() label: string;
  @Input() type = 'text';

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }
  
  registerOnTouched(fn: any): void {
  }
}
