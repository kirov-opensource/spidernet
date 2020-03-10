import React from "react";
import Jobs from '../components/Job';
import Home from '../components/Home';
import { Menu } from 'antd';
const { SubMenu } = Menu;
import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link
} from "react-router-dom";

// Some folks find value in a centralized route config.
// A route config is just data. React is great at mapping
// data into components, and <Route> is a component.

// Our route config is just an array of logical "routes"
// with `path` and `component` props, ordered the same
// way you'd do inside a `<Switch>`.
const routes = [
    {
        path: "/home",
        component: Home
    },
    {
        path: "/jobs",
        component: Jobs,
        // routes: [
        //   {
        //     path: "/jobs/:id",
        //     component: Cart
        //   }
        // ]
    }
];

export default function RouteConfigExample() {
    return (
        <Router>
            <Menu
                mode="inline"
                defaultSelectedKeys={['1']}
                defaultOpenKeys={['BasicData']}
                style={{ height: '100%', borderRight: 0 }}
                inlineCollapsed={this.state.collapsed}
            >
                <SubMenu
                    key="BasicData"
                    title={
                        <span>
                            <UserOutlined />
                            Basic data
                                    </span>
                    }
                >
                    <Menu.Item key="Home">Home</Menu.Item>
                    <Menu.Item key="Jobs">Jobs</Menu.Item>
                    <Menu.Item key="Templates">Templates</Menu.Item>
                </SubMenu>
            </Menu>
        </Router>
    );
}

// A special wrapper for <Route> that knows how to
// handle "sub"-routes by passing them in a `routes`
// prop to the component it renders.
function RouteWithSubRoutes(route) {
    return (
        <Route
            path={route.path}
            render={props => (
                // pass the sub-routes down to keep nesting
                <route.component {...props} routes={route.routes} />
            )}
        />
    );
}

function Sandwiches() {
    return <h2>Sandwiches</h2>;
}

function Tacos({ routes }) {
    return (
        <div>
            <h2>Tacos</h2>
            <ul>
                <li>
                    <Link to="/tacos/bus">Bus</Link>
                </li>
                <li>
                    <Link to="/tacos/cart">Cart</Link>
                </li>
            </ul>

            <Switch>
                {routes.map((route, i) => (
                    <RouteWithSubRoutes key={i} {...route} />
                ))}
            </Switch>
        </div>
    );
}

function Bus() {
    return <h3>Bus</h3>;
}

function Cart() {
    return <h3>Cart</h3>;
}