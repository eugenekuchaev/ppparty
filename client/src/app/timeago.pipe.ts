import { Pipe, PipeTransform } from '@angular/core';
import { TimeagoPipe } from 'ngx-timeago';

@Pipe({
  name: 'timeago',
  pure: false // setting pure to false ensures that the pipe updates when the input changes
})
export class CustomTimeagoPipe extends TimeagoPipe implements PipeTransform {
  transform(value: string): string {
    return super.transform(value);
  }
}
