import { ChangeDetectionStrategy, Component, input, ViewEncapsulation } from '@angular/core';
import LayoutComponent from "../layout/layout.component";

@Component({
  selector: "app-blank",
  imports: [LayoutComponent],
  templateUrl: './blank.component.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class BlankComponent {
readonly pageTitle = input.required<string>();
}
