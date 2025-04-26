import { ChangeDetectionStrategy, Component, computed, inject, signal, ViewEncapsulation } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { BreadcrumbService } from '../../services/breadcrumb.service';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  imports: [RouterOutlet,RouterLink,CommonModule],
  templateUrl: './layout.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,// Performans için değişiklik algılamayı optimize eder
  encapsulation: ViewEncapsulation.None// Bileşenin stillerini global yapar
})
export default class LayoutComponent {
  breadcrumbs = computed(()=> this.#breadcrumb.data());
  time = signal<string>("");

  #breadcrumb = inject(BreadcrumbService);
  #date = inject(DatePipe);
  #router = inject(Router);

  constructor(){
    setInterval(()=>{
    this.time.set(this.#date.transform(new Date(), "dd.MM.yyyy HH:mm:ss")!)
    },1000)
  }

  logout(){
    localStorage.clear();
    this.#router.navigateByUrl("/login");
  }
}
