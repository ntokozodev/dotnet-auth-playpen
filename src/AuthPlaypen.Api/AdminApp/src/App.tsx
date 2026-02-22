import { Route, Router } from "@solidjs/router";
import { MainLayout } from "@/layouts/MainLayout";
import { Applications } from "@/pages/Applications";
import { Dashboard } from "@/pages/Dashboard";
import { EditApplication } from "@/pages/EditApplication";
import { EditScope } from "@/pages/EditScope";
import { Scopes } from "@/pages/Scopes";

export default function App() {
  return (
    <Router base="/admin">
      <Route component={MainLayout}>
        <Route path="/" component={Dashboard} />
        <Route path="/applications" component={Applications} />
        <Route path="/applications/:id/edit" component={EditApplication} />
        <Route path="/scopes" component={Scopes} />
        <Route path="/scopes/:id/edit" component={EditScope} />
      </Route>
    </Router>
  );
}
